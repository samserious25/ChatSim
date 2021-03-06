# ChatSim

APK в корне.

О программе.
1. Так как это не полноценный чат-клиент, а всего лишь визуальное представление подобного чата, можно было бы пойти более простым путем и ограничиться логикой добавления спрайтов сообщений в скролл. Но, так как это все же симулятор, я попытался в очень упрощённом виде описать логику работы подобной программы с использованием воображаемых клиентов и иденичного сервера, общение между ними заменить событиями. Все сообщения от каждого клиента приходят на сервер и рассылаются всем клиентам. Наше окно - окно нашего клиента.
2. Сервер. Принимает подключения, запросы от клиентов на переход в канал и непосредственно сами сообщения, который потом рассылает всем участникам канала.
3. Клиент. Подключется к серверу, подключается к каналам (их может быть сколько угодно, но логика сильно упрощена до списка с именами каналов), есть активный канал, в котором он находится непосредственно сейчас.
4. Сообщение. В нем само сообщение, ник пользователя и его роль. Оно, естественно не пакуется в байты для передачи.
5. Канал. Класс со списком расположившихся в нем пользователей, удалает и добавляет клиентов.
6. Мир клиентов. Здесь располагается редактируемый список каналов с включенным в каждый из них список клиентов. На старте создаётся сервер, создаются все клиенты и каждый из них подключается к каналам, которые указаны в настраеваемом списке.
7. UI. Подписан на сообщения от мира клиентов, также отправляет сообщение миру клиентов с запросом отослать сообщение. Здесь также происходит удаление сообщений (список сообщений не ведётся на сервере, ведётся тут, учитывая то, что это все же симулятор).
8. Информация о клиенте. Представлена скриптуемых объектом, в котором 3 поля: имя, роль, аватар. Соответственно этими объектами мы и формируем участников каналов.

О фишках.
Я постарался выдержать визуал и логику максимально соответствующими описанию работы программы, но решил добавить несколько фишек на свое усмотрение.

1. Можно перейти в список всех актуальных каналов и перейти на любой из них.
2. При подключении к чату, выводится сообщение о подключившемся клиенте.
3. В правом верхнем углу отображается название канала.
4. Для того, чтобы все работало, в инспекторе нужно указать себя и свой основной канал. Можно включить эмуляцию активности канала. Все кроме нас, соответственно, заходят и выходят, присылают сообщения со случайным текстом из базы сообщений.
5. Сообщения в чатах сохраняются на время сессии.

О проблемах.
Клавиатура андроид в юнити полностью блокирует нажатие на элементы UI (может есть и какая-то другая причина), поэтому отправить сообщение возможно только после сворачивания клавиатуры. Единственный вариант - написать что-то своё (клавиатуру или InpurField), но это займет время.

О доработках.
Естественно, можно добавить пул всех объектов, которые мы добавляем и убавляем, готовым пользоваться не стал. Можно также было бы ограничиться различными префабами для каждого вида баблов сообщений, но я сделал префаб универсальным.
